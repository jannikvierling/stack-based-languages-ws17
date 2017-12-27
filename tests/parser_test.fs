require ../parser.fs
require ../util.fs
require ../clauselist.fs

begin-dimacs

1 2 0
-1 -2 0

2 clauses

end-dimacs

dup 1 2 0 2 times clause.insert_literal swap clauselist.contains_clause? .

dup -1 -2 0 2 times clause.insert_literal swap clauselist.contains_clause? .

list.length 2 = .
