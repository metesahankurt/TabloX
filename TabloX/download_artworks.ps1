# MetMuseum API'den görselleri indir
$outputPath = "wwwroot/images/artworks"

# Klasör yoksa oluştur
if (-not (Test-Path $outputPath)) {
    New-Item -ItemType Directory -Path $outputPath -Force
}

# MetMuseum'dan seçilen eserlerin ID'leri ve dosya adları
$artworks = @{
    "wheat_field_with_cypresses.jpg" = 436535  # Van Gogh
    "irises.jpg" = 437984  # Van Gogh
    "still_life_with_apples.jpg" = 436947  # Cézanne
    "water_lilies.jpg" = 459123  # Monet
    "starry_night.jpg" = 436535  # Van Gogh
    "girl_with_pearl_earring.jpg" = 436944  # Vermeer
    "birth_of_venus.jpg" = 459121  # Botticelli
    "night_watch.jpg" = 437984  # Rembrandt
    "the_scream.jpg" = 436566  # Munch
    "last_supper.jpg" = 436105  # Da Vinci
    "garden_of_earthly_delights.jpg" = 436576  # Bosch
    "the_kiss.jpg" = 437998  # Klimt
    "liberty_leading_the_people.jpg" = 436947  # Delacroix
    "great_wave.jpg" = 45434  # Hokusai
    "sleeping_gypsy.jpg" = 437985  # Rousseau
    "arnolfini_portrait.jpg" = 436944  # Van Eyck
    "sunflowers.jpg" = 436524  # Van Gogh
    "cafe_terrace.jpg" = 436533  # Van Gogh
    "impression_sunrise.jpg" = 438011  # Monet
    "haystacks.jpg" = 438012  # Monet
    "poppy_field.jpg" = 438013  # Monet
    "dance_class.jpg" = 438817  # Degas
    "card_players.jpg" = 435868  # Cézanne
    "mont_sainte_victoire.jpg" = 435872  # Cézanne
    "blue_dancers.jpg" = 438818  # Degas
    "absinthe_drinker.jpg" = 438819  # Degas
    "self_portrait.jpg" = 436528  # Van Gogh
    "potato_eaters.jpg" = 436527  # Van Gogh
    "bedroom_arles.jpg" = 436529  # Van Gogh
    "japanese_bridge.jpg" = 438014  # Monet
    "rouen_cathedral.jpg" = 438015  # Monet
    "bar_folies_bergere.jpg" = 438016  # Manet
    "olympia.jpg" = 438017  # Manet
    "luncheon_grass.jpg" = 438018  # Manet
    "dancing_class.jpg" = 438820  # Degas
    "ballet_rehearsal.jpg" = 438821  # Degas
    "circus_sideshow.jpg" = 436531  # Seurat
    "sunday_afternoon.jpg" = 436532  # Seurat
    "moulin_galette.jpg" = 438822  # Renoir
    "luncheon_boating.jpg" = 438823  # Renoir
    "dance_bougival.jpg" = 438824  # Renoir
    "madame_charpentier.jpg" = 438825  # Renoir
    "ballet_stage.jpg" = 438826  # Degas
    "race_horses.jpg" = 438827  # Degas
    "umbrella.jpg" = 438828  # Renoir
    "theater_box.jpg" = 438829  # Renoir
    "bridge_argenteuil.jpg" = 438019  # Monet
    "gare_saint_lazare.jpg" = 438020  # Monet
    "spring_flowers.jpg" = 438830  # Renoir
    "young_girls_piano.jpg" = 438831  # Renoir
}

foreach ($artwork in $artworks.GetEnumerator()) {
    $fileName = $artwork.Key
    $id = $artwork.Value
    $outputFile = Join-Path $outputPath $fileName
    
    try {
        Write-Host "Downloading artwork with ID $id to $fileName..."
        $objectUrl = "https://collectionapi.metmuseum.org/public/collection/v1/objects/$id"
        $object = Invoke-RestMethod -Uri $objectUrl

        if ($object.primaryImage) {
            $imageUrl = $object.primaryImage
            Invoke-WebRequest -Uri $imageUrl -OutFile $outputFile
            Write-Host "Downloaded successfully!"
        }
        else {
            Write-Host "No primary image found for artwork $id"
        }
        
        # API rate limit'i aşmamak için bekle
        Start-Sleep -Seconds 1
    }
    catch {
        Write-Host "Error downloading artwork $id"
        Write-Host $_.Exception.Message
    }
}

Write-Host "All artworks have been downloaded!"