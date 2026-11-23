/* libc_min.c -- what GCC may call on its own for block copies and clears. */
#include <stddef.h>
void *memcpy(void *d, const void *s, size_t n) {
    unsigned char *a = d; const unsigned char *b = s;
    while (n--) *a++ = *b++;
    return d;
}
void *memset(void *d, int v, size_t n) {
    unsigned char *a = d;
    while (n--) *a++ = (unsigned char)v;
    return d;
}
