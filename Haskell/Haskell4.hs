-- Ugnius Padolskis Informatika 4Kursas 1Grupė
module H4 where

-- Exercise 1: 
data GTree a = Leaf a
  | GNode [GTree a]
  deriving (Show, Eq)


depth :: GTree a -> Int
depth (Leaf _)   = 1
depth (GNode ts) = 1 + maximum (map depth ts)

occurs :: Eq a => a -> GTree a -> Bool
occurs x (Leaf y)   = x == y
occurs x (GNode ts) = any (occurs x) ts

mapLeaves :: (a -> b) -> GTree a -> GTree b
mapLeaves f (Leaf x)   = Leaf (f x)
mapLeaves f (GNode ts) = GNode (map (mapLeaves f) ts)


-- Exercise 2: 
type Var = Char
type Ops a = [a] -> a
type Valuation a = Var -> a

data Expr a
  = Lit a
  | EVar Var
  | Op (Ops a) [Expr a]

eval :: Valuation a -> Expr a -> a
eval _   (Lit x)      = x
eval val (EVar v)     = val v
eval val (Op f es)    = f (map (eval val) es)


-- Exercise 3:
data RegExp = Empty
  | Eps
  | CharR Char
  | Seq RegExp RegExp
  | Alt RegExp RegExp
  | Star RegExp
  deriving Show

option :: RegExp -> RegExp
option p = Alt p Eps

plus :: RegExp -> RegExp
plus p = Seq p (Star p)



-- Exrcise 4: 
data Result a = OK a
  | Error String
  deriving Show

composeResult
  :: (a -> Result b)
  -> (b -> Result c)
  -> a
  -> Result c
composeResult f g x =
  case f x of
    Error msg -> Error msg
    OK y      -> g y

-- Exercise 5:
primes :: [Integer]
primes = sieve [2..]
  where
    sieve (p:xs) = p : sieve [x | x <- xs, x `mod` p /= 0]
    sieve []     = []

isGoldbach :: Integer -> Bool
isGoldbach n =
  any (\p -> (n - p) `elem` takeWhile (<= n) primes)
      (takeWhile (<= n) primes)

goldbach :: Integer -> Bool
goldbach n =
  all isGoldbach [x | x <- [4..n], even x]

-- Exercise 6: 
data Stream a = Cons a (Stream a)

streamToList :: Stream a -> [a]
streamToList (Cons x xs) = x : streamToList xs

streamIterate :: (a -> a) -> a -> Stream a
streamIterate f x = Cons x (streamIterate f (f x))

streamInterleave :: Stream a -> Stream a -> Stream a
streamInterleave (Cons x xs) (Cons y ys) =
  Cons x (Cons y (streamInterleave xs ys))
