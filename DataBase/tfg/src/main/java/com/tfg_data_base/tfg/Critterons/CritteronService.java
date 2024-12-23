package com.tfg_data_base.tfg.Critterons;

import java.util.List;
import java.util.Optional;
import org.springframework.stereotype.Service;
import com.tfg_data_base.tfg.GameInfo.GameInfoService;
import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
public class CritteronService {
    private final CritteronRepository critteronRepository;
    private final GameInfoService gameInfoService;

    public void save(Critteron critteron) {
        if (critteron.getId() == null || critteron.getId().isEmpty()) {
            critteron.setId(null);
        }
        critteronRepository.save(critteron);
        gameInfoService.addCritteron(critteron.getId());
    }

    public List<Critteron> findAll() {
        return critteronRepository.findAll();
    }

    public Optional<Critteron> findById(String id) {
        return critteronRepository.findById(id);
    }

    public void deleteById(String id) {
        critteronRepository.deleteById(id);
        gameInfoService.removeCritteron(id);
    }
}
