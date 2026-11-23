# gba/Makefile -- the Generation 3 save editor, as a GBA multiboot program.
#
#   make test                        the portable core, on the host
#   make test ROM=x.gba SYM=x.sym    also against a ROM and pret's symbols
#   make                             editor.mb (arm-none-eabi-gcc)
#   make font                        src/font_data.c from tools/font.txt
CORE := src/g3save.c src/mon.c src/g3text.c src/growth.c src/romdata.c src/mem.c
HOSTCC ?= cc

.PHONY: all test font clean
all: build/editor.mb

build:
	mkdir -p build

build/test_core: test/test_core.c $(CORE) src/*.h | build
	$(HOSTCC) -std=gnu11 -O1 -g -Wall -Wextra -fsanitize=address,undefined -o $@ test/test_core.c $(CORE)

test: build/test_core
	./build/test_core $(if $(ROM),"$(ROM)" "$(SYM)")

font:
	python3 tools/mkfont.py tools/font.txt src/font_data.c

clean:
	rm -rf build
