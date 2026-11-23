/* hw.h -- the screen as a 30x20 grid of text, and the keys. */
#ifndef HW_H
#define HW_H
#include <stdint.h>

enum { PAL_TEXT, PAL_SEL, PAL_WARN, PAL_DIM, PAL_GOOD, PAL_HEAD };
enum { KEY_A = 1, KEY_B = 2, KEY_SELECT = 4, KEY_START = 8, KEY_RIGHT = 16, KEY_LEFT = 32,
       KEY_UP = 64, KEY_DOWN = 128, KEY_R = 256, KEY_L = 512 };

void hw_init(void);
void vsync(void);
void cls(void);
void put(int x, int y, int pal, const char *s);        /* clipped at the right edge */
void putn(int x, int y, int pal, const char *s, int w);/* padded or cut to w cells */
void line_pal(int y, int pal);                        /* recolour a whole row */
void fmt_u(char *out, uint32_t v, int width);         /* right-aligned, space padded */
void fmt_hex(char *out, uint32_t v, int digits);
/* Keys newly pressed this frame, with auto-repeat for held d-pad and L/R. */
uint16_t keys_poll(void);
uint16_t keys_held(void);
#endif
