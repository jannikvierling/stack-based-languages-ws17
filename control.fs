: GUARD ( Compilation: -- orig; Run-time: f -- )
    POSTPONE IF ; immediate

: END ( Compilation: orig -- )
    POSTPONE EXIT POSTPONE ENDIF ; immediate
