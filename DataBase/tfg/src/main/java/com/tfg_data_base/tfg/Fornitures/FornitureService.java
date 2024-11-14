package com.tfg_data_base.tfg.Fornitures;

import java.util.List;
import java.util.Optional;

import org.springframework.stereotype.Service;

import com.tfg_data_base.tfg.GameInfo.GameInfoService;

import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
public class FornitureService {
    private final FornitureRepository fornitureRepository;
    private final GameInfoService gameInfoService;

    public void save(Forniture forniture) {
        if (forniture.getId() == null || forniture.getId().isEmpty()) {
            forniture.setId(null);
        }
        fornitureRepository.save(forniture);
        gameInfoService.addForniture(forniture.getId());
    }

    public List<Forniture> findAll() {
        return fornitureRepository.findAll();
    }

    public Optional<Forniture> findById(String id) {
        return fornitureRepository.findById(id);
    }

    public void deleteById(String id) {
        fornitureRepository.deleteById(id);
        gameInfoService.removeForniture(id);
    }
}
