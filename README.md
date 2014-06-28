PKHeX
=====

Pokémon X/Y SAV/PKX file editor, programmed in C#.
Supports binary file loading of:

	Decrypted Save files (1MB) 
	Individual entity files (232/260 byte)
	
Data is then displayed in a meaningful view, and can be edited and saved back to Binary data.
For save files, re-saving is possible (using SHA256 hashes; AES MAC cannot be emulated).

The interface is translatable with resource/external text files so that different languages can be supported.

Note: I started learning C# by making this program, so certain implementations may look odd ;)
