#import "AnyLanguageTextRenderer.h"
#import "UnityAppController.h"

@implementation AnyLanguageTextRenderer

#define UIColorFromRGB(rgbValue) [UIColor colorWithRed:((float)((rgbValue & 0xFF0000) >> 16))/255.0 \
                                                green:((float)((rgbValue & 0xFF00) >> 8))/255.0 \
                                                 blue:((float)(rgbValue & 0xFF))/255.0 alpha:1.0]

- (UIColor *)colorFromHexString:(NSString *)hexString {
    NSString *cleanString = [hexString stringByReplacingOccurrencesOfString:@"#" withString:@""];
    if ([cleanString length] != 6) return UIColorFromRGB(0x000000);
    unsigned int hex;
    NSScanner *scanner = [NSScanner scannerWithString:cleanString];
    if (![scanner scanHexInt:&hex]) return UIColorFromRGB(0x000000);
    return UIColorFromRGB(hex);
}

- (NSAttributedString *)attributedStringFromSimpleHTML:(NSString *)htmlString font:(UIFont*)font defaultColor:(UIColor*)defaultColor {
    // Replace <br> with newline
    NSString *textWithLineBreaks = [htmlString stringByReplacingOccurrencesOfString:@"<br>" withString:@"\n"];
    textWithLineBreaks = [textWithLineBreaks stringByReplacingOccurrencesOfString:@"<br/>" withString:@"\n"];
    textWithLineBreaks = [textWithLineBreaks stringByReplacingOccurrencesOfString:@"<br />" withString:@"\n"];

    NSMutableAttributedString *result = [[NSMutableAttributedString alloc] initWithString:@"" attributes:@{ NSFontAttributeName: font, NSForegroundColorAttributeName: defaultColor }];
    
    NSError *regexError = nil;
    NSRegularExpression *regex = [NSRegularExpression regularExpressionWithPattern:@"<span style=\"color:(#[0-9a-fA-F]{6})\">(.*?)</span>" options:NSRegularExpressionDotMatchesLineSeparators error:&regexError];
    
    __block NSInteger lastIndex = 0;
    NSArray *matches = [regex matchesInString:textWithLineBreaks options:0 range:NSMakeRange(0, textWithLineBreaks.length)];
    
    for (NSTextCheckingResult *match in matches) {
        // Text before this span
        NSRange beforeRange = NSMakeRange(lastIndex, match.range.location - lastIndex);
        if (beforeRange.length > 0) {
            NSString *beforeText = [textWithLineBreaks substringWithRange:beforeRange];
            beforeText = [beforeText stringByReplacingOccurrencesOfString:@"<[^>]+>" withString:@"" options:NSRegularExpressionSearch range:NSMakeRange(0, beforeText.length)];
            NSAttributedString *beforeAttr = [[NSAttributedString alloc] initWithString:beforeText attributes:@{ NSFontAttributeName: font, NSForegroundColorAttributeName: defaultColor }];
            [result appendAttributedString:beforeAttr];
        }
        
        // Span text
        NSString *hexColor = [textWithLineBreaks substringWithRange:[match rangeAtIndex:1]];
        NSString *innerText = [textWithLineBreaks substringWithRange:[match rangeAtIndex:2]];
        NSAttributedString *spanAttr = [[NSAttributedString alloc] initWithString:innerText attributes:@{ NSFontAttributeName: font, NSForegroundColorAttributeName: [self colorFromHexString:hexColor] }];
        [result appendAttributedString:spanAttr];
        
        lastIndex = match.range.location + match.range.length;
    }
    
    // Remaining text after last span
    if (lastIndex < textWithLineBreaks.length) {
        NSString *remaining = [textWithLineBreaks substringFromIndex:lastIndex];
        remaining = [remaining stringByReplacingOccurrencesOfString:@"<[^>]+>" withString:@"" options:NSRegularExpressionSearch range:NSMakeRange(0, remaining.length)];
        NSAttributedString *remainingAttr = [[NSAttributedString alloc] initWithString:remaining attributes:@{ NSFontAttributeName: font, NSForegroundColorAttributeName: defaultColor }];
        [result appendAttributedString:remainingAttr];
    }
    
    return result;
}

- (NSString *)imageFromString:(NSString *)string
                    textwidth:(float)textWidth
                   textHeight:(float)textHeight
                     fontSize:(int)fontSize
                    alignment:(NSString*)alignmentStr
                    fontStyle:(NSString*)fontStyleStr
                        color:(NSString*)colorStr
                  strokeWidth:(int)strokeWidth
                  strokeColor:(NSString*)strokeClrStr
              textWidthOffset:(float)textWidthOffset
                   isLongText:(BOOL)isLongText
            virticalalignment:(NSString*)virticalAlignmentStr {

    //NSLog(@"[YGText] here1  %@", string);
    
    
    float widthOffset = 1.0;
    if (isLongText && string.length >= 35) widthOffset = textWidthOffset;
    CGSize size = CGSizeMake(widthOffset * textWidth, textHeight);
    UIGraphicsBeginImageContextWithOptions(size, NO, 0);
    
    NSString *alias = [[alignmentStr stringByTrimmingCharactersInSet:[NSCharacterSet whitespaceAndNewlineCharacterSet]] lowercaseString];
    if ([alias isEqualToString:@"center center"] || [alias isEqualToString:@"centre centre"] || [alias isEqualToString:@"center_center"] || [alias isEqualToString:@"centre_centre"]) {
        alignmentStr = @"Center";
        virticalAlignmentStr = @"Center";
    }
    
    //NSLog(@"[YGText] here2");
    
    NSMutableParagraphStyle *paragraphStyle = [[NSMutableParagraphStyle alloc] init];
    paragraphStyle.lineBreakMode = NSLineBreakByWordWrapping;
    paragraphStyle.alignment = NSTextAlignmentCenter;
    if ([alignmentStr isEqualToString:@"Left"]) paragraphStyle.alignment = NSTextAlignmentLeft;
    if ([alignmentStr isEqualToString:@"Right"]) paragraphStyle.alignment = NSTextAlignmentRight;
    
    UIFont *font = [UIFont systemFontOfSize:fontSize];
    if ([fontStyleStr isEqualToString:@"Bold"]) font = [UIFont boldSystemFontOfSize:fontSize];
    if ([fontStyleStr isEqualToString:@"Italic"]) font = [UIFont italicSystemFontOfSize:fontSize];
    
    UIColor *textColor = [self colorFromHexString:colorStr];
    UIColor *strokeColor = [self colorFromHexString:strokeClrStr];
    NSDictionary *attributes = @{
        NSFontAttributeName: font,
        NSBackgroundColorAttributeName: [UIColor clearColor],
        NSForegroundColorAttributeName: textColor,
        NSParagraphStyleAttributeName: paragraphStyle,
        NSStrokeWidthAttributeName: @(strokeWidth),
        NSStrokeColorAttributeName: strokeColor
    };
    
    //NSLog(@"[YGText] here3");
    
    // -------- Manual HTML Parsing --------
    BOOL isHtml = [string containsString:@"<span"];
    NSAttributedString *attributedString = nil;
    
    if (isHtml) {
        attributedString = [self attributedStringFromSimpleHTML:string font:font defaultColor:textColor];
        isHtml = YES;
    }
    
    //NSLog(@"[YGText] here4");
    
    CGRect textRect;
    if (isHtml && attributedString && attributedString.length > 0) {
        textRect = [attributedString boundingRectWithSize:size options:NSStringDrawingUsesLineFragmentOrigin | NSStringDrawingUsesFontLeading context:nil];
    } else {
        textRect = [string boundingRectWithSize:size options:NSStringDrawingUsesLineFragmentOrigin | NSStringDrawingUsesFontLeading attributes:attributes context:nil];
    }
    
    //NSLog(@"[YGText] here5");
    
    // Alignment
    if ([alignmentStr isEqualToString:@"Left"]) textRect.origin.x = 0;
    else if ([alignmentStr isEqualToString:@"Right"]) textRect.origin.x = size.width - textRect.size.width;
    else textRect.origin.x = (size.width - textRect.size.width) / 2.0;
    
    if ([virticalAlignmentStr isEqualToString:@"Top"]) textRect.origin.y = 0;
    else if ([virticalAlignmentStr isEqualToString:@"Bottom"]) textRect.origin.y = size.height - textRect.size.height;
    else textRect.origin.y = (size.height - textRect.size.height) / 2.0;
    
    //NSLog(@"[YGText] here6");
    
    if (isHtml && attributedString && attributedString.length > 0) {
        [attributedString drawInRect:textRect];
    } else {
        [string drawInRect:textRect withAttributes:attributes];
    }
    
    UIImage *image = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    NSData *data = UIImagePNGRepresentation(image);
    //NSLog(@"[YGText] here7");
    
    return [data base64EncodedStringWithOptions:NSDataBase64EncodingEndLineWithLineFeed];
}

@end

// ---------------- C String helpers ----------------

NSString* CreateNSStringFor (const char* string) {
    if (string) return [NSString stringWithUTF8String:string];
    return [NSString stringWithUTF8String:""];
}

char* makeStringChar(const char* str) {
    if (!str) return NULL;
    char* res = (char*)malloc(strlen(str)+1);
    strcpy(res, str);
    return res;
}

extern "C" {
char* _getImageBytesFromString(const char* string, float textWidth, float textHeight, int fontSize, const char *alignment, const char *fontStyle, const char *color, int outLineWidth, const char *outLineColor, float textWidthOffset, BOOL isLongText, const char *virticalalignment) {
    
    id AnyLanguageTextRendererPlugin = [[AnyLanguageTextRenderer alloc] init];
    NSString *objString = CreateNSStringFor(string);
    NSString *alignmentStr = CreateNSStringFor(alignment);
    NSString *fontStyleStr = CreateNSStringFor(fontStyle);
    NSString *colorStr = CreateNSStringFor(color);
    NSString *strokeColorStr = CreateNSStringFor(outLineColor);
    NSString *vericalalignmentStr = CreateNSStringFor(virticalalignment);

    NSString* textStr = [[NSString alloc] initWithString:[AnyLanguageTextRendererPlugin
        imageFromString:objString
        textwidth:textWidth
        textHeight:textHeight
        fontSize:fontSize
        alignment:alignmentStr
        fontStyle:fontStyleStr
        color:colorStr
        strokeWidth:outLineWidth
        strokeColor:strokeColorStr
        textWidthOffset:textWidthOffset
        isLongText:isLongText
        virticalalignment:vericalalignmentStr]];
    
    const char* s = [textStr cStringUsingEncoding:NSUTF8StringEncoding];
    char* c = makeStringChar(s);
    return c;
}
}

