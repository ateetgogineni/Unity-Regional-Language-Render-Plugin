package com.yesgnome.anylanguagerenderer;

import android.content.res.AssetManager;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.graphics.Typeface;
import android.text.Html;
import android.text.Layout;
import android.text.Spanned;
import android.text.SpannedString;
import android.text.StaticLayout;
import android.text.TextPaint;
import android.util.Base64;

import com.unity3d.player.UnityPlayer;        // ← requires Unity as a dependency

import java.io.ByteArrayOutputStream;

/**
 * Utility to render a text string into an image and return it as a Base64-encoded PNG.
 * Optimized for high-quality, sharp text rendering.
 *
 * Author: Govinda Rajulu Yella
 * Last update: 02-Jul-2025
 */
public class MainActivity {

    /** High-quality supersampling factor */
    private static final int SCALE_FACTOR = 4;

    /**
     * Renders @text into a bitmap (PNG) and returns the Base64 string.
     *
     * @param fontPath   Either:
     *                   • a relative path inside the APK assets, e.g. "Kalam-Regular.ttf" or "fonts/Kalam-Regular.ttf", or
     *                   • an absolute file path (for downloaded fonts).
     */
    public static String getImageFromText(String text,
                                          int textWidth,
                                          int textSize,
                                          int quality,
                                          String textAlignment,
                                          String textStyle,
                                          String color,
                                          String verticalAlignment,
                                          int textHeight,
                                          String fontPath) {

        if (text == null || text.trim().isEmpty()) return "";

        try {
            // ── 1  Prepare scaled metrics ───────────────────────────────────────
            int scaledSize   = textSize   * SCALE_FACTOR;
            int scaledWidth  = textWidth  * SCALE_FACTOR;
            int scaledHeight = textHeight * SCALE_FACTOR;

            // ── 2  Configure paint ─────────────────────────────────────────────
            TextPaint paint = new TextPaint(Paint.ANTI_ALIAS_FLAG
                                            | Paint.LINEAR_TEXT_FLAG
                                            | Paint.SUBPIXEL_TEXT_FLAG);
            paint.setStyle(Paint.Style.FILL);
            paint.setTextSize(scaledSize);
            paint.setColor(parseColorOrDefault(color, Color.BLACK));
            paint.setTypeface(getTypefaceForStyle(fontPath, textStyle));

            // ── 3  Dynamic sizing: shrink text until it fits within scaledHeight ─
            Layout.Alignment align = getAlignment(textAlignment);
            Spanned clean       = stripHtml(text);
            TextPaint dynPaint  = new TextPaint(paint);
            StaticLayout layout = null;
            // Try sizes stepping down by 2px (on the supersampled scale)
            for (int s = scaledSize; s > 2; s -= 2) {
                dynPaint.setTextSize(s);
                layout = StaticLayout.Builder
                            .obtain(clean, 0, clean.length(), dynPaint, scaledWidth)
                            .setAlignment(align)
                            .setLineSpacing(0f, 1f)
                            .setIncludePad(true)
                            .build();
                if (layout.getHeight() <= scaledHeight) {
                    // found a size that fits
                    paint = dynPaint;
                    break;
                }
            }
            // Fallback to original size if nothing fit
            if (layout == null) {
                paint.setTextSize(scaledSize);
                layout = StaticLayout.Builder
                            .obtain(clean, 0, clean.length(), paint, scaledWidth)
                            .setAlignment(align)
                            .setLineSpacing(0f, 1f)
                            .setIncludePad(true)
                            .build();
            }

            // ── 4  Draw to high-res bitmap ────────────────────────────────────
            Bitmap hi = Bitmap.createBitmap(scaledWidth, scaledHeight, Bitmap.Config.ARGB_8888);
            Canvas  c  = new Canvas(hi);

            // vertical alignment
            float yOffset = 0f;
            int bitmapH = hi.getHeight();
            int textH   = layout.getHeight();

            switch ((verticalAlignment == null ? "TOP" : verticalAlignment).toUpperCase()) {
                case "CENTER": yOffset = (bitmapH - textH) / 2f; break;
                case "BOTTOM": yOffset =  bitmapH - textH;      break;
            }

            c.save();
            c.translate(0, yOffset);
            layout.draw(c);
            c.restore();

            // ── 5  Down-sample to final size ───────────────────────────────────
            Bitmap finalBmp = Bitmap.createScaledBitmap(hi, textWidth, textHeight, true);
            hi.recycle();

            return bitmapToBase64(finalBmp);

        } catch (Exception ex) {
            ex.printStackTrace();
            return "";
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────────────────────────────────────

    private static String bitmapToBase64(Bitmap bmp) {
        if (bmp == null) return "";
        ByteArrayOutputStream out = new ByteArrayOutputStream();
        bmp.compress(Bitmap.CompressFormat.PNG, 100, out);      // quality ignored for PNG
        bmp.recycle();
        return Base64.encodeToString(out.toByteArray(), Base64.NO_WRAP);
    }

    private static int parseColorOrDefault(String hex, int def) {
        if (hex == null || hex.length() < 3) return def;
        try { return Color.parseColor(hex); } catch (IllegalArgumentException e) { return def; }
    }

    /** Asset-aware Typeface loader */
    private static Typeface getTypefaceForStyle(String fontPath, String textStyle) {
        Typeface base = Typeface.DEFAULT;

        if (fontPath != null && !fontPath.trim().isEmpty()) {
            boolean fromAssets = !fontPath.startsWith("/") && !fontPath.startsWith("jar:");
            try {
                if (fromAssets) {
                    AssetManager am = UnityPlayer.currentActivity.getAssets();
                    base = Typeface.createFromAsset(am, fontPath);
                } else {
                    base = Typeface.createFromFile(fontPath);
                }
            } catch (Exception e) {
                e.printStackTrace();
                base = Typeface.DEFAULT;
            }
        }

        int style = Typeface.NORMAL;
        if (textStyle != null) {
            switch (textStyle.toUpperCase()) {
                case "BOLD":        style = Typeface.BOLD;         break;
                case "ITALIC":      style = Typeface.ITALIC;       break;
                case "BI":
                case "BOLDITALIC":  style = Typeface.BOLD_ITALIC;  break;
            }
        }
        return Typeface.create(base, style);
    }

    private static Layout.Alignment getAlignment(String a) {
        if (a == null) return Layout.Alignment.ALIGN_CENTER;
        switch (a.toUpperCase()) {
            case "LEFT":  return Layout.Alignment.ALIGN_NORMAL;
            case "RIGHT": return Layout.Alignment.ALIGN_OPPOSITE;
            default:      return Layout.Alignment.ALIGN_CENTER;
        }
    }

    private static Spanned stripHtml(String html) {
        if (html == null) return SpannedString.valueOf("");
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.N)
            return Html.fromHtml(html, Html.FROM_HTML_MODE_LEGACY);
        else
            return Html.fromHtml(html);
    }
}
