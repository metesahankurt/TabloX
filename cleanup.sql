-- Boş kategorileri bul
SELECT c.Id, c.Name, COUNT(a.Id) as ArtworkCount
FROM Categories c
LEFT JOIN Artworks a ON c.Id = a.CategoryId
GROUP BY c.Id, c.Name
ORDER BY ArtworkCount;

-- Eseri olmayan sanatçıları bul
SELECT ar.Id, ar.Name, COUNT(aw.Id) as ArtworkCount
FROM Artists ar
LEFT JOIN Artworks aw ON ar.Id = aw.ArtistId
GROUP BY ar.Id, ar.Name
ORDER BY ArtworkCount;

-- Kategorilere göre eser dağılımı
SELECT c.Name as CategoryName, COUNT(a.Id) as ArtworkCount, 
       GROUP_CONCAT(DISTINCT ar.Name) as Artists
FROM Categories c
LEFT JOIN Artworks a ON c.Id = a.CategoryId
LEFT JOIN Artists ar ON a.ArtistId = ar.Id
GROUP BY c.Name
ORDER BY c.Name; 