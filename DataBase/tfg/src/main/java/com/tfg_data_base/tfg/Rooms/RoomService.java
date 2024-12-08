package com.tfg_data_base.tfg.Rooms;

import java.util.List;
import java.util.Optional;

import org.springframework.stereotype.Service;

import com.tfg_data_base.tfg.GameInfo.GameInfoService;

import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
public class RoomService {
    private final RoomRepository roomRepository;
    private final GameInfoService gameInfoService;

    public void save(Room room) {
        if (room.getId() == null || room.getId().isEmpty()) {
            room.setId(null);
        }
        roomRepository.save(room);
        gameInfoService.addRoom(room.getId());
    }

    public List<Room> findAll() {
        return roomRepository.findAll();
    }

    public Optional<Room> findById(String id) {
        return roomRepository.findById(id);
    }

    public void deleteById(String id) {
        roomRepository.deleteById(id);
        gameInfoService.removeRoom(id);
    }
}
