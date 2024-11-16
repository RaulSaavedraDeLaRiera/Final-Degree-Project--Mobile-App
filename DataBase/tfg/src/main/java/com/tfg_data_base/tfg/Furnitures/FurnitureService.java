package com.tfg_data_base.tfg.Furnitures;

import java.util.List;
import java.util.Optional;

import org.springframework.stereotype.Service;

import com.tfg_data_base.tfg.GameInfo.GameInfoService;

import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
public class FurnitureService {
    private final FurnitureRepository furnitureRepository;
    private final GameInfoService gameInfoService;

    public void save(Furniture furniture) {
        if (furniture.getId() == null || furniture.getId().isEmpty()) {
            furniture.setId(null);
        }
        furnitureRepository.save(furniture);
        gameInfoService.addFurniture(furniture.getId());
    }

    public List<Furniture> findAll() {
        return furnitureRepository.findAll();
    }

    public Optional<Furniture> findById(String id) {
        return furnitureRepository.findById(id);
    }

    public void deleteById(String id) {
        furnitureRepository.deleteById(id);
        gameInfoService.removeFurniture(id);
    }
}
