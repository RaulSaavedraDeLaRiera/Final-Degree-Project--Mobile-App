package com.tfg_data_base.tfg.Marks;

import java.util.List;
import java.util.Optional;
import org.springframework.stereotype.Service;
import com.tfg_data_base.tfg.GameInfo.GameInfoService;
import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
public class MarkService {
    private final MarkRepository markRepository;
    private final GameInfoService gameInfoService;

    public void save(Mark mark) {
        if (mark.getId() == null || mark.getId().isEmpty()) {
            mark.setId(null);
        }
        markRepository.save(mark);
        gameInfoService.addMark(mark.getId());
    }

    public List<Mark> findAll() {
        return markRepository.findAll();
    }

    public Optional<Mark> findById(String id) {
        return markRepository.findById(id);
    }

    public void deleteById(String id) {
        markRepository.deleteById(id);
        gameInfoService.removeMark(id);
    }
}
