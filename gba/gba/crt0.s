@ crt0.s -- the editor's multiboot header and start.
@
@ Adapted from gbagate's rom/crt0_mb.s (GPL-3.0, HelaFaye/gbagate), which
@ explains each choice at length. In short:
@  - the BIOS copies the image to 0x02000000 and jumps to the branch at 0xC0;
@  - the Nintendo logo at 0x04 is left zero: it is Nintendo's artwork, and
@    the sender (gbagate's board) fills it from a ROM you own;
@  - start is at 0xE8, not 0xE4: mGBA reads a branch of 28 bytes at 0xC0 as
@    "not multiboot" (GBAIsMB);
@  - .data is linked where it runs, so it needs no copying; .bss is zeroed.
    .section .gba_crt0, "ax"
    .arm
    .align 2
    .global _start
_start:
    b       .Lstart                 @ 0x00
    .space  156, 0                  @ 0x04: Nintendo logo (the sender fills it)
    .byte   'P','K','H','e','X',' ','G','B','A',0,0,0  @ 0xA0: title
    .byte   'C','P','K','E'         @ 0xAC: game code
    .byte   '0','0'                 @ 0xB0: maker
    .byte   0x96, 0x00, 0x00        @ 0xB2: fixed, unit, device
    .space  7, 0
    .byte   0x00, 0x00              @ 0xBC: version, complement (the sender fixes it)
    .space  2, 0
    b       .Lstart                 @ 0xC0: where the BIOS enters
    .byte   0x00, 0x00              @ 0xC4: boot mode, slave ID (the BIOS writes them)
    .space  26, 0
    b       .Lstart                 @ 0xE0: JOYBUS entry
    .space  4, 0                    @ 0xE4: keeps .Lstart at 0xE8
    .align 2
.Lstart:
    mov     r0, #0x04000000
    str     r0, [r0, #0x208]        @ IME off: the editor polls, it takes no interrupts
    ldr     sp, =__stack_top
    ldr     r0, =__bss_start__
    ldr     r1, =__bss_end__
    mov     r2, #0
.Lbss:
    cmp     r0, r1
    strlo   r2, [r0], #4
    blo     .Lbss
    ldr     r0, =main
    mov     lr, pc
    bx      r0
.Lhang:
    b       .Lhang

    .section .mbfoot, "a"
    .align 2
    .word   0x424D4247              @ "GBMB": the image never ends in 0x00 or 0xFF
