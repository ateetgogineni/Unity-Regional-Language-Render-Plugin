#import <UIKit/UIKit.h>

@interface AnyLanguageTextRenderer : NSObject

- (NSString *)imageFromString:(NSString *)string
                    textwidth:(float)textWidth
                   textHeight:(float)textHeight
                     fontSize:(int)fontSize
                    alignment:(NSString *)alignmentStr
                    fontStyle:(NSString *)fontStyleStr
                        color:(NSString *)colorStr
                  strokeWidth:(int)strokeWidth
                  strokeColor:(NSString *)strokeClrStr
              textWidthOffset:(float)textWidthOffset
                   isLongText:(BOOL)isLongText
            virticalalignment:(NSString *)virticalAlignmentStr;

@end
