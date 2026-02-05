-- Ugnius Padolskis Informatika 4 kursas 1 grupė

module h3 where

-- Exercise 1
data Shape = Circle Float Float Float | Rectangle Float Float Float Float 


overlaps :: Shape -> Shape -> Bool
overlaps (Circle r1 x1 y1) (Circle r2 x2 y2) = 
    distance <= r1 + r2
    where distance = sqrt ((x2 - x1)^2 + (y2 - y1)^2)

overlaps (Rectangle w1 h1 x1 y1) (Rectangle w2 h2 x2 y2) =
    xOverlap && yOverlap
    where
        left1 = x1 - w1/2
        right1 = x1 + w1/2
        top1 = y1 + h1/2
        bottom1 = y1 - h1/2
        
        left2 = x2 - w2/2
        right2 = x2 + w2/2
        top2 = y2 + h2/2
        bottom2 = y2 - h2/2
        
        xOverlap = left1 < right2 && right1 > left2
        yOverlap = bottom1 < top2 && top1 > bottom2

overlaps (Circle r x1 y1) (Rectangle w h x2 y2) = 
    distancee <= r
    where
        left = x2 - w/2
        right = x2 + w/2
        top = y2 + h/2
        bottom = y2 - h/2
        
        closestX = max left (min x1 right)
        closestY = max bottom (min y1 top)
        
        distancee = sqrt ((x1 - closestX)^2 + (y1 - closestY)^2)

overlaps rectangle@(Rectangle _ _ _ _) circle@(Circle _ _ _) = 
    overlaps circle rectangle 



-- Exercise 2
-- su filtru
anyF :: (a -> Bool) -> [a] -> Bool
anyF p xs = not (null (filter p xs))

allF :: (a -> Bool) -> [a] -> Bool
allF p xs = null (filter (not . p) xs)

--  su map ir foldr
anyMF :: (a -> Bool) -> [a] -> Bool
anyMF p = foldr (||) False . map p

allMF :: (a -> Bool) -> [a] -> Bool
allMF p = foldr (&&) True  . map p



-- Exercise 3
unzipas :: [(a,b)] -> ([a],[b])
unzipas = foldr stepas ([],[])
  where
    stepas (x,y) (xs,ys) = (x:xs, y:ys)



-- Exercise 4
--  su map ir sudėtimi
length1 :: [a] -> Int
length1 = sum . map (const 1)

-- su foldu ir lamba
length2 :: [a] -> Int
length2 = foldr (\_ n -> n + 1) 0



-- Exercise 5
limitas :: Integer -> [Integer] -> Integer
limitas bound = go 0
  where
    go acc (x:xs)
      | acc + x > bound = acc
      | otherwise       = go (acc + x) xs
    go acc [] = acc

ff :: Integer -> [Integer] -> Integer
ff maxNum = limitas maxNum . map (*10) . filter (>= 0)



-- Exercise 6
-- su map ir sudėtimi
total :: (Integer -> Integer) -> Integer -> Integer
total f n = sum (map f [0..n])

-- Exercise 7
-- rekursija
iter1 :: Integer -> (a -> a) -> (a -> a)
iter1 n f
  | n <= 0    = id
  | n == 1    = f
  | otherwise = f . iter (n-1) f

-- su replicate ir foldr
iter2 :: Integer -> (a -> a) -> (a -> a)
iter2 n f
  | n <= 0    = id
  | otherwise = foldr (.) id (replicate (fromIntegral n) f)



-- Exercise 8
splits :: [a] -> [([a],[a])]
splits xs = [ splitAt i xs | i <- [0 .. length xs] ]
