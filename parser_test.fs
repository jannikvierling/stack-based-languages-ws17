require parser.fs
require util.fs
require clauselist.fs

begin-dimacs

1 2 0
-1 -2 0

2 clauses

end-dimacs

dup 1 2 0 2 times insert-literal swap contains-clause .

dup -1 -2 0 2 times insert-literal swap contains-clause .

list-length 2 = .
