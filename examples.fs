require literal.fs
require llclause.fs
require llclauseset.fs
require resolution.fs
require helpers.fs

1 2 3
0
3 times insert-literal
constant c0

1 -2 4
0
3 times insert-literal
constant c1

-1 -2 4
0
3 times insert-literal
constant c2

c1 c0
0
2 times insert-clause
constant s0

c2 c1 c0
0
3 times insert-clause
constant s1

1 2
0
2 times insert-literal
constant d0

-1 2
0
2 times insert-literal
constant d1

-2
0
insert-literal
constant d2

d2 d1 d0
0
3 times insert-clause
constant t0

\ t0 is-sat
