0 constant input

variable seen
0 seen !

variable working
0 working !

: append-seen ( clause -- )
    seen @ append-if-new \ append would suffice
    seen ! ;

\ Returns true if working is empty, false otherwise.
: working-empty ( -- f )
    working @ 0= ;

\ Returns true if the empty clause is in working, false otherwise.
: working-box ( -- f )
    0 working @ contains-clause ;

\ Returns true if the termination condition is reached, false otherwise.
: terminate ( -- f )
    working-empty working-box or ;

\ Removes the first clause from working and returns it.
: next-clause ( -- clause )
    working @ pop-clause swap working ! ;

: shift-clause ( -- clause )
    next-clause dup append-seen ;

: is-not-seen ( clause -- f )
    seen @ contains-clause invert ;

: dispatch-new ( clause_1 ... clause_n n -- )
    0 U+DO
        dup is-not-seen IF dup working @ append-if-new working ! ENDIF drop
    LOOP ;

: process-clause ( clause -- )
    { clause } seen @ BEGIN
        dup WHILE
            dup clauselist-clause @ clause resolve-all
            dispatch-new
            list-next @
    REPEAT drop ;

: refute ( -- )
    BEGIN
        terminate invert WHILE
            shift-clause
            process-clause
    REPEAT ;

