: literal.= ( l1 l2 -- flag )
    \ Tests two literals for equality.
    \
    \ If l1 and l2 are equal, then flag is true, otherwise flag is
    \ false.
    = ;

: literal.dual ( literal -- dual )
    \ Computes the dual of a literal.
    negate ;

: literal.is_dual? ( l1 l2 -- flag )
    \ Tests whether two literals are duals.
    \
    \ If l1 and l2 are dual, then flag is true, otherwise flag is
    \ false.
    literal.dual literal.= ;

: literal.< { l1 l2 -- flag }
    \ Compares two literals.
    \
    \ If flag is true, then l1 is considered to be less than
    \ l2. Otherwise if flag is false, then l1 is considered greater or
    \ equal than l2.
    l1 abs l2 abs <
    l1 abs l2 abs =
    l1 l2 <
    and or ;

: literal.> ( l1 l2 -- flag )
    \ Compares two literals.
    \
    \ If flag is true, then l1 is considered to be greater than
    \ l2. Otherwise if flag is false, then l1 is considered less or
    \ equal than l2.
    2dup literal.< -rot literal.= or invert ;
