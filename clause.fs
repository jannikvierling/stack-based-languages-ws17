require control.fs
require literal.fs
require list.fs

list%
    cell% field clause.literal
end-struct clause%

: clause.deallocator ( node -- )
    drop ;

: clause.free ( clause -- )
    ['] clause.deallocator list.free ;

: clause.next list.next ;

: clause.size list.length ;

: clause.new_node ( literal next -- node )
    \ Creates a new clause node.
    clause% %alloc tuck clause.next ! tuck clause.literal ! ;

: clause.copy ( clause1 -- clause2 ) recursive
    \ Copies a clause.
    { clause } clause 0= GUARD
        0 END
    clause clause.literal @
    clause clause.next @ clause.copy
    clause.new_node ;

: clause.remove_literal ( literal clause1 -- clause2 ) recursive
    \ Removes a literal from a clause.
    { literal clause }
    clause 0= GUARD
        0 END
    clause clause.literal @ literal = GUARD
         clause clause.next @ clause list.free_node END
    literal clause clause.next @ clause.remove_literal
    clause clause.next !
    clause ;

: clause.insert_literal ( literal clause1 -- clause2 ) recursive
    \ Inserts a literal in a clause if it does not occur.
    \
    \ The resulting clause "clause2" is ordered.
    { literal clause }
    clause 0= GUARD
        literal 0 clause.new_node END
    clause clause.literal @ literal literal-greater GUARD
        literal clause clause.new_node END
    clause clause.literal @ literal literal-less GUARD
        literal clause clause.next @ clause.insert_literal
        clause clause.next !
        clause END        
    clause ;

: clause.show_literal' ( clause -- )
    clause.literal @ 1 .r ;

: clause.show ( clause -- )
    \ Prints a clause.
    { clause } [Char] ] clause bl ['] clause.show_literal' [Char] [ list.show ;

: clause.merge.select' { clause1 clause2 -- c1' c2' l }
    clause1 clause.literal @ clause2 clause.literal @ literal-equal GUARD
         clause1 clause.next @ clause2 clause.next @ clause1 clause.literal @ END
    clause1 clause.literal @ clause2 clause.literal @ literal-less GUARD
         clause1 clause.next @ clause2 clause1 clause.literal @ END
    clause1 clause2 clause.next @ clause2 clause.literal @ ;

: clause.merge' recursive { result clause1 clause2 -- } 
	clause1 0= GUARD
		clause2 clause.copy result clause.next ! END
    clause2 0= GUARD
        clause1 clause.copy result clause.next ! END
    clause1 clause2 clause.merge.select'
    0 clause.new_node dup result clause.next ! -rot
    clause.merge' ;

: clause.merge ( clause1 clause2 -- clause )
    0 0 clause.new_node dup 2swap clause.merge'
	clause.next @ ; \ todo take care of initial dummy element

: clause.equal? ( clause1 clause2 -- f ) recursive
    \ Compares two clauses for equality.
    \
    \ "clause1", "clause2": Adresses to clauses.
    \ "f": true if the clauses are equal, false otherwise.
    { clause1 clause2 }
    clause1 0= GUARD
        clause2 0= IF true ELSE false ENDIF END
    clause2 0= GUARD
        false END
    clause1 clause.literal @ clause2 clause.literal @ =
    clause1 clause.next @ clause2 clause.next @
    clause.equal? and ;

: clause.contains_literal? ( literal clause -- f)
    ['] clause.literal ['] = 2swap list.search ;

: clause.resolve ( literal clause1 clause2 -- resolvent )
    \ Resolves two clauses upon a literal.
    \
    \ "literal": The resolved upon literal; this is a literal of
    \ clause1 and its dual must appear in clause2.
    { literal clause1 clause2 }
    literal clause1 clause.copy clause.remove_literal { c1 } c1
    literal negate clause2 clause.copy clause.remove_literal { c2 } c2
    clause.merge c1 c2 clause.free clause.free ;

: clause.resolve_full ( clause1 clause2 -- res_1 ... res_n n )
    \ Resolves two clauses upon all resolvable literals.
    { clause1 clause2 }
    0 clause1 BEGIN
        dup WHILE
            dup clause.literal @ negate clause2 clause.contains_literal? IF
                dup clause.literal @ clause1 clause2 clause.resolve
                rot 1+ rot
            ENDIF
            clause.next @
    REPEAT drop ;

: clause.subsumes_clause? ( c_2 c_1 -- f )
    \ Checks if a given clauses subsumes another clause.
    \
    \ Returns true if c_2 subsumes c_1, false otherwise.
    { c_1 } BEGIN dup 0<> WHILE
            dup clause.literal @ c_1 clause.contains_literal? invert IF
                drop false EXIT ENDIF
            list.next @
    REPEAT drop true ;

: clause.subsumed_by? ( c_1 c_2 -- f )
    \ Checks if a given clause is subsumed by another clause.
    \
    \ Returns true if c_1 is subsumed by c_2, false otherwise.
    swap clause.subsumes_clause? ;

: clause.compare_by_length ( clause1 clause2 -- f )
    \ Compares two clauses by length.
    \
    \ Returns true if the length of the first clause is strictly less
    \ than the length of the second clause.
    list.length swap list.length swap < ;
