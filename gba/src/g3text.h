/* g3text.h -- the Generation 3 Western character set, to and from the
 * editor's font codes (ASCII, plus 128.. for the symbols in font.txt). */
#ifndef G3TEXT_H
#define G3TEXT_H
#include <stdint.h>

#define G3_EOS 0xFF
#define FONT_MALE 128
#define FONT_FEMALE 129
#define FONT_E_ACUTE 130
#define FONT_ELLIPSIS 131
#define FONT_CURSOR 132
#define FONT_BLOCK 133

/* One game character as a font code; '?' for one the font lacks. */
uint8_t g3_to_font(uint8_t c);
/* One font code as a game character; 0 when the game has none for it. */
uint8_t g3_from_font(uint8_t f);
/* Up to max game characters (stopping at EOS) into out as font codes,
 * NUL-terminated: out holds max + 1. Returns the length. */
int g3_decode(const uint8_t *s, int max, char *out);
/* The characters a name may be edited through, in order. */
extern const uint8_t g3_name_chars[];
extern const int g3_name_chars_count;
#endif
