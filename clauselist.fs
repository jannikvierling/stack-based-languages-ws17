
require list.fs
require clause.fs

list%
    cell% field clauselist.clause
end-struct clauselist%

: clauselist.deallocator ( node -- )
    \ Frees the contents of a clause list node.
    clauselist.clause clause.free ;

: clauselist.free ( clauselist -- )
    \ Frees the given clause list.
    ['] clauselist.deallocator list.free ;

: clauselist.contains_clause? ( clause* list<clause*> -- f )
    \ Checks whether a clause occurs in a list.
    \
    \ Returns true if the clause occurs in the list, false otherwise.
    ['] clauselist.clause ['] clause.equal? 2swap list.search ;

: clauselist.subsumes_clause? ( clause* list<clauselist*> -- f )
    \ Checks whether a clause is subsumed by a clause list.
    \
    \ Returns true if the clause is subsumed by a clause in the list,
    \ false otherwise.
    ['] clauselist.clause ['] clause.subsumed_by? 2swap list.search ;

: clauselist.show_clause ( clauselist -- )
    \ Prints the clause reference by a clause list node.
    clauselist.clause @ clause.show ;

: clauselist.show ( clauselist -- )
    \ Prints a clause list on the screen.
    { list } [Char] } list 10 ['] clauselist.show_clause [Char] { list.show ;

: clauselist.pop ( list<clause*> -- list<clause*> clause* )
    \ Pops the head of a given clause list.
    list.pop swap clauselist.clause @ ;

: clauselist.new_node { clause next -- list }
    \ Creates a new clause list node.
    \
    \ The new node references "clause" and its successor is "next.
    clauselist% %alloc { new }
    clause new clauselist.clause !
    next new list.next ! new ;

: clauselist.append ( clause clauselist -- clauselist )
    \ Appends a clause to a clause list.
    \
    \ Creates a new node at the end of the given list. If the given
    \ list is empty the address of the new node is returned, otherwise
    \ the address of the given list is returned.
    dup list.is_empty? GUARD
        drop 0 clauselist.new_node END
    tuck list.last swap 0 clauselist.new_node swap list.next ! ;

: clauselist.append_new ( clause clauselist -- clauselist )
    \ Appends a clause to a list.
    \
    \ The clause is appended only if it does not already occur in the
    \ list. See 'append'.
    2dup clauselist.contains_clause? GUARD
        nip END
    clauselist.append ;
    
: clauselist.insert_ordered.iterate?' ( list clause < -- f )
    \ Checks if another iteration is to be carried out.
    { list clause < } list 0= GUARD
        false END
    list clauselist.clause @ clause < execute ;

: clauselist.insert_node' ( prev current clause list -- list )
    \ Inserts a node at the given position.
    \
    \ Inserts the node clause right after prev. Returns the new node
    \ if prev is empty, otherwise list is returned.
    { list } swap clauselist.new_node { prev new }
    prev 0= GUARD
        new END
    new prev list.next ! list ;
    
: clauselist.insert_ordered' ( list comparator clause -- list )
    \ Inserts a new clause relative to some ordering.
    \
    \ The ordering is determined by the comparator.
    { list < clause } 0 list BEGIN          ( loop invariant: prev current -- )
        dup clause < clauselist.insert_ordered.iterate?' WHILE
            nip dup list.next @
    REPEAT clause list clauselist.insert_node' ;
                    
: clauselist.insert_clause_ordered ( comparator clause list -- list' )
    \ Inserts a clause into a list relative to some ordering.
    \
    \ If the clause to be inserted occurs in list, then it is not
    \ inserted.
    2dup clauselist.contains_clause? GUARD
        nip nip END
    rot rot clauselist.insert_ordered' ;
    
: clauselist.insert_clause ( clause list -- list' )
    \ Inserts clause in a clause list w.r.t. to the lenght ordering.
    ['] clause.compare_by_length -rot clauselist.insert_clause_ordered ;