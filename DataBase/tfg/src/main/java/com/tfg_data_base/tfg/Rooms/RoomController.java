package com.tfg_data_base.tfg.Rooms;

import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import lombok.RequiredArgsConstructor;

import java.util.List;

import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;

@RestController
@RequestMapping("/api/v1")
@RequiredArgsConstructor
public class RoomController {

    private final RoomService furnitureService;

    @PostMapping("/room")
    public void save(@RequestBody Room furniture) {
        furnitureService.save(furniture);
    }

    @GetMapping("/room")
    public List<Room> findAll() {
        return furnitureService.findAll();
    }

    @GetMapping("/room/{id}")
    public Room findById(@PathVariable String id) {
        return furnitureService.findById(id).get();
    }

    @DeleteMapping("/room/{id}")
    public void deleteById(@PathVariable String id) {
        furnitureService.deleteById(id);
    }

    @PutMapping("/room")
    public void update(@RequestBody Room furniture) {
        furnitureService.save(furniture);
    }
}
