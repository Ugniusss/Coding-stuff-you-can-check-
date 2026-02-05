-- Ugnius Padolskis Informatika 4kursas


module Haskell2 where
import Data.Char (isLetter, toUpper)
-- Exercise 1
average :: [Float] -> Float
average x = sum x / fromIntegral (length x)

-- Exercise 2
divides :: Integer -> [Integer]
divides n = helper n 1
  where
    helper n k
      | k > n = []
      | n `mod` k == 0 = k : helper n (k + 1)
      | otherwise = helper n (k + 1)

dividesComp :: Integer -> [Integer]
dividesComp n = [x | x <- [1..n], n `mod` x == 0]

isPrime :: Integer -> Bool
isPrime n = dividesComp n == [1, n]

-- Exercise 3
prefix :: String -> String -> Bool
prefix [] _ = True
prefix _ [] = False
prefix (x:xs) (y:ys) = (x == y) && prefix xs ys

substring :: String -> String -> Bool
substring [] _ = True
substring _ [] = False
substring xs ys@(y:ys')
  | prefix xs ys = True
  | otherwise = substring xs ys'

-- Exercise 4
permut :: [Integer] -> [Integer] -> Bool
permut [] [] = True
permut _ [] = False
permut [] _ = False
permut (x:xs) ys
  | x `elem` ys = permut xs (remove x ys)
  | otherwise = False
  where
    remove _ [] = []
    remove a (b:bs)
      | a == b = bs
      | otherwise = b : remove a bs

-- Exercise 5
capitalise :: String -> String
capitalise str = [toUpper x | x <- str, isLetter x]

-- Exercise 6
itemTotal :: [(String, Float)] -> [(String, Float)]
itemTotal [] = []
itemTotal ((n,p):xs) =
  let same = [(a,b) | (a,b) <- xs, prefix n a && prefix a n]
      rest = [(a,b) | (a,b) <- xs, not (prefix n a && prefix a n)]
      total = p + sum [b | (_,b) <- same]
  in (n,total) : itemTotal rest
  
itemDiscount :: String -> Integer -> [(String, Float)] -> [(String, Float)]
itemDiscount _ _ [] = []
itemDiscount name disc ((n,p):xs)
  | prefix name n && prefix n name = (n, p * (1 - fromIntegral disc / 100)) : itemDiscount name disc xs
  | otherwise = (n,p) : itemDiscount name disc xs
