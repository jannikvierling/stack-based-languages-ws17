
require list.fs
require clause.fs

list%
    cell% field clauselist-clause
end-struct clauselist%

: contains-clause ( clause* list<clause*> -- f )
    \ Checks whether a clause occurs in a list.
    \
    \ Returns true if the clause occurs in the list, false otherwise.
    ['] clauselist-clause ['] clauses-equal 2swap list-search ;

: is_subsumed? ( clause* list<clauselist*> -- f )
    \ Checks whether a clause is subsumed by a clause list.
    \
    \ Returns true if the clause is subsumed by a clause in the list,
    \ false otherwise.
    ['] clauselist-clause [']  subsumed? 2swap list-search ;

: show' ( clauselist -- )
    \ Prints the clause reference by a clause list node.
    clauselist-clause @ show-clause ;

: show-clauselist ( clauselist -- )
    \ Prints a clause list on the screen.
    { list } [Char] } list 10 ['] show' [Char] { list-show ;

: pop-clause ( list<clause*> -- list<clause*> clause* )
    \ Pops the head of a given clause list.
    list-pop swap clauselist-clause @ ;

: new-clause-list-node { clause next -- list }
    \ Creates a new clause list node.
    \
    \ The new node references "clause" and its successor is "next.
    clauselist% %alloc { new }
    clause new clauselist-clause !
    next new list-next ! new ;

: append ( clause clauselist -- clauselist )
    \ Appends a clause to a clause list.
    \
    \ Creates a new node at the end of the given list. If the given
    \ list is empty the address of the new node is returned, otherwise
    \ the address of the given list is returned.
    dup is_emptylist? GUARD
        drop 0 new-clause-list-node END
    tuck last-node swap 0 new-clause-list-node swap list-next ! ;

: append-new ( clause clauselist -- clauselist )
    \ Appends a clause to a list.
    \
    \ The clause is appended only if it does not already occur in the
    \ list. See 'append'.
    2dup contains-clause GUARD
        nip END
    append ;

: length-comparator ( clause1 clause2 -- f )
    \ Compares two clauses by length.
    \
    \ Returns true if the length of the first clause is strictly less
    \ than the length of the second clause.
    list-length swap list-length swap < ;
    
: io-iterate? ( list clause < -- f )
    \ Checks if another iteration is to be carried out.
    { list clause < } list 0= GUARD
        false END
    list clauselist-clause @ clause < execute ;

: insert-node ( prev current clause list -- list )
    \ Inserts a node at the given position.
    \
    \ Inserts the node clause right after prev. Returns the new node
    \ if prev is empty, otherwise list is returned.
    { list } swap new-clause-list-node { prev new }
    prev 0= GUARD
        new END
    new prev list-next ! list ;
    
: insert-ordered' ( list comparator clause -- list )
    \ Inserts a new clause relative to some ordering.
    \
    \ The ordering is determined by the comparator.
    { list < clause } 0 list BEGIN          ( loop invariant: prev current -- )
        dup clause < io-iterate? WHILE
            nip dup list-next @
    REPEAT clause list insert-node ;
                    
: insert-ordered ( comparator clause list -- list' )
    \ Inserts a clause into a list relative to some ordering.
    \
    \ If the clause to be inserted occurs in list, then it is not
    \ inserted.
    2dup contains-clause GUARD
        nip nip END
    rot rot insert-ordered' ;

: insert-by-length ( clause list -- list' )
    \ Inserts a clause w.r.t. to the length ordering.
    ['] length-comparator -rot insert-ordered ;

: insert-clause ( clause list -- list' )
    \ Inserts clause in a clause list w.r.t. to the lenght ordering.
    insert-by-length ;