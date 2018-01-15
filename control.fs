
: GUARD ( Compilation: -- orig; Run-time: f -- )
    \ This construct is quite similar to concept of guards in the
    \ programming language Haskell.
    \
    \ At run time it takes a flag, if the flag is equivalent to the
    \ boolean true, then the code following the word GUARD up to the
    \ associated occurrence of END is executed, after that the
    \ execution of the word ends. If the flag is equivalent to false,
    \ then the code after the associated occurence of the word END is
    \ executed.
    POSTPONE IF ; immediate

: END ( Compilation: orig -- )
    \ Delimits a GUARD construct (See GUARD).
    POSTPONE EXIT POSTPONE ENDIF ; immediate
