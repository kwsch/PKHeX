/* hw.c -- see hw.h. Mode 0, background 0: the font in character block 0,
 * the text map in screen block 31. Each palette bank colours text (1) and
 * its background (2). */
#include "hw.h"

#define REG_DISPCNT  (*(volatile uint16_t *)0x04000000)
#define REG_VCOUNT   (*(volatile uint16_t *)0x04000006)
#define REG_BG0CNT   (*(volatile uint16_t *)0x04000008)
#define REG_KEYINPUT (*(volatile uint16_t *)0x04000130)
#define PAL_BG       ((volatile uint16_t *)0x05000000)
#define VRAM_TILES   ((volatile uint32_t *)0x06000000)
#define MAP          ((volatile uint16_t *)0x0600F800)

extern const uint32_t font_tiles[134 * 8];

static uint16_t rgb(int r, int g, int b) { return (uint16_t)(r | g << 5 | b << 10); }

void hw_init(void) {
    REG_DISPCNT = 0x0080;                       /* forced blank while loading */
    for (int k = 0; k < 134 * 8; k++) VRAM_TILES[k] = font_tiles[k];
    const uint16_t navy = rgb(2, 4, 10);
    const uint16_t cols[][2] = {
        {rgb(31, 31, 31), navy},                /* PAL_TEXT */
        {rgb(2, 4, 10), rgb(31, 27, 8)},        /* PAL_SEL: dark on yellow */
        {rgb(31, 12, 10), navy},                /* PAL_WARN */
        {rgb(17, 19, 23), navy},                /* PAL_DIM */
        {rgb(12, 31, 14), navy},                /* PAL_GOOD */
        {rgb(31, 31, 31), rgb(8, 10, 20)},      /* PAL_HEAD */
    };
    for (unsigned b = 0; b < sizeof cols / sizeof cols[0]; b++) {
        PAL_BG[b * 16 + 1] = cols[b][0];
        PAL_BG[b * 16 + 2] = cols[b][1];
    }
    PAL_BG[0] = navy;
    REG_BG0CNT = 31 << 8;                       /* char block 0, screen block 31, 4bpp */
    cls();
    REG_DISPCNT = 0x0100;                       /* mode 0, BG0 */
}

void vsync(void) {
    while (REG_VCOUNT >= 160) {}
    while (REG_VCOUNT < 160) {}
}

void cls(void) {
    for (int k = 0; k < 32 * 32; k++) MAP[k] = ' ';
}

void put(int x, int y, int pal, const char *s) {
    for (; *s && x < 30; x++, s++)
        if (x >= 0 && y >= 0 && y < 20) MAP[y * 32 + x] = (uint16_t)((uint8_t)*s | pal << 12);
}

void putn(int x, int y, int pal, const char *s, int w) {
    for (int k = 0; k < w; k++) {
        const char c = *s ? *s++ : ' ';
        if (x + k < 30 && y >= 0 && y < 20) MAP[y * 32 + x + k] = (uint16_t)((uint8_t)c | pal << 12);
    }
}

void line_pal(int y, int pal) {
    for (int x = 0; x < 30; x++) MAP[y * 32 + x] = (uint16_t)((MAP[y * 32 + x] & 0x0FFF) | pal << 12);
}

void fmt_u(char *out, uint32_t v, int width) {
    char tmp[12];
    int n = 0;
    do { tmp[n++] = (char)('0' + v % 10); v /= 10; } while (v);
    int k = 0;
    for (; k < width - n; k++) out[k] = ' ';
    while (n) out[k++] = tmp[--n];
    out[k] = 0;
}

void fmt_hex(char *out, uint32_t v, int digits) {
    for (int k = 0; k < digits; k++) out[k] = "0123456789ABCDEF"[v >> (4 * (digits - 1 - k)) & 15];
    out[digits] = 0;
}

static uint16_t s_prev, s_count;

uint16_t keys_held(void) { return (uint16_t)(~REG_KEYINPUT & 0x3FF); }

uint16_t keys_poll(void) {
    const uint16_t now = keys_held();
    uint16_t pressed = now & (uint16_t)~s_prev;
    const uint16_t rep = KEY_UP | KEY_DOWN | KEY_LEFT | KEY_RIGHT | KEY_L | KEY_R;
    if (now & rep & s_prev) {
        if (++s_count > 18 && (s_count & 3) == 0) pressed |= now & rep;   /* 0.3 s, then 15/s */
    } else {
        s_count = 0;
    }
    s_prev = now;
    return pressed;
}
