require clause.fs
require clauselist.fs

\ Holds the address of the seen set.
variable seen
0 seen !

\ Holds the address of the working set.
variable working
0 working !

: append-seen ( clause -- )
    \ Appends a clause to the seen set.
    \
    \ "clause" The clause to append to the seen set.
    seen @ append-if-new
    seen ! ;

: working-empty ( -- f )
    \ Checks whether the working set is empty.
    \
    \ "f" true if working is empty, false otherwise.
    working @ 0= ;

: working-box ( -- f )
    \ Checks whether the empty clause occurs in the working set.
    \
    \ "f" true if the empty clause is in working, false otherwise.
    0 working @ contains-clause ;

: terminate ( -- f )
    \ Checks whether the termination condition is reached.
    \
    \ "f" true if either the empty clause occurs in the working set,
    \ or if the working set is empty; false otherwise
    working-empty working-box or ;

: next-clause ( -- clause )
    \ Removes the first clause from the working clauses.
    \
    \ "clause" The clause removed from the working set.
    working @ pop-clause swap working ! ;

: shift-clause ( -- clause )
    \ Moves one clause from the head of the working set to the end of the
    \ seen set.
    \ "clause" The clause that was moved.
    next-clause dup append-seen ;

: is-not-seen ( clause -- f )
    \ Checks whether a clause is in the sen set.
    \
    \ "clause" The clause to check.
    \ "f" true if the clause is not in the seen set, false otherwise.
    seen @ contains-clause invert ;

: dispatch-new ( clause_1 ... clause_n n -- )
    \ Dispatches a group of new clauses.
    \
    \ New clauses are appended to the working set if they neither
    \ occur in the seen set nor in the working set.
    \ "clause_1 ... clause_n" The clauses to be inserted to working.
    \ "n" The number of clauses to consider.
    0 U+DO
        dup is-not-seen IF dup working @ append-if-new working ! ENDIF drop
    LOOP ;

: process-clause ( clause -- )
    \ Processes a given clause.
    \
    \ The clause is resolved fully resolved against all clauses in the seen set.
    \ Resulting clauses are inserted into the working set if they are not redundant.
    \ "clause" The clause to process.
    { clause } seen @ BEGIN
        dup WHILE
            dup clauselist-clause @ clause resolve-all
            dispatch-new
            list-next @
    REPEAT drop ;

: refute ( -- )
    \ Tries to refute the clauses in the working set.
    \
    \ After the algorithm terminates (seen; working) represents a
    \ resolution deduction from working.
    BEGIN
        terminate invert WHILE
            shift-clause
            process-clause
    REPEAT ;

