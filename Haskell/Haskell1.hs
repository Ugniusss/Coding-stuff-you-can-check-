-- Ugnius Padolskis Informatika 4 kursas 1 grupė


module Haskell1 where

-- Exercise 1
nAnd1 :: Bool -> Bool -> Bool
nAnd1 a b = not (a && b)

nAnd2 :: Bool -> Bool -> Bool
nAnd2 a b
  | a && b    = False
  | otherwise = True

nAnd3 :: Bool -> Bool -> Bool
nAnd3 True True   = False
nAnd3 True False  = True
nAnd3 False True  = True
nAnd3 False False = True

-- Exercise 2
nDigits :: Integer -> Int
nDigits x
  | x < 0     = length (show (abs x))
  | otherwise = length (show x)

-- Exercise 3
nRoots :: Float -> Float -> Float -> Int
nRoots a b c
  | a == 0.0  = error "negali but 0"
  | disc > 0  = 2
  | disc == 0 = 1
  | otherwise = 0
  where
    disc = b^2 - 4 * a * c

-- Exercise 4
smallerRoot :: Float -> Float -> Float -> Float
smallerRoot a b c
  | a == 0.0  = error "negali but 0"
  | disc < 0  = error "ngl saknis"
  | otherwise = ((-b) - sqrt disc) / (2 * a)
  where
    disc = b^2 - 4 * a * c

largerRoot :: Float -> Float -> Float -> Float
largerRoot a b c
  | a == 0.0  = error "negali but 0"
  | disc < 0  = error "ngl saknis"
  | otherwise = ((-b) + sqrt disc) / (2 * a)
  where
    disc = b^2 - 4 * a * c

-- Exercise 5
power2 :: Integer -> Integer
power2 n
  | n < 0     = 0
  | n == 0    = 1
  | otherwise = 2 * power2 (n - 1)

-- Exercise 6
mult :: Integer -> Integer -> Integer
mult m n
  | m == 0 || n == 0 = 0
  | n > 0            = m + mult m (n - 1)
  | n < 0            = negate (mult m (negate n))



-- Exercise 7
prod :: Integer -> Integer -> Integer
prod m n
  | m > n     = error "turi but m <= n"
  | m == n    = n
  | otherwise = m * prod (m + 1) n

factorial :: Integer -> Integer
factorial n
  | n < 0     = error "neigiamas"
  | n == 0    = 1
  | otherwise = prod 1 n
