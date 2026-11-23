#include "mem.h"
void mem_copy(void *dst, const void *src, size_t n) {
    uint8_t *d = dst; const uint8_t *s = src;
    while (n--) *d++ = *s++;
}
void mem_set(void *dst, int v, size_t n) {
    uint8_t *d = dst;
    while (n--) *d++ = (uint8_t)v;
}
int mem_equal(const void *a, const void *b, size_t n) {
    const uint8_t *x = a, *y = b;
    while (n--) if (*x++ != *y++) return 0;
    return 1;
}
