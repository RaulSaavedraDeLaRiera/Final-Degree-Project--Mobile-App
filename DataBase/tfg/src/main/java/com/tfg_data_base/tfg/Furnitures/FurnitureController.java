package com.tfg_data_base.tfg.Furnitures;

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
public class FurnitureController {

    private final FurnitureService furnitureService;

    @PostMapping("/furniture")
    public void save(@RequestBody Furniture furniture){
        furnitureService.save(furniture);
    }

    @GetMapping("/furniture")
    public List<Furniture> findAll(){
        return furnitureService.findAll();
    }
    
    @GetMapping("/furniture/{id}")
    public Furniture findById(@PathVariable String id){
        return furnitureService.findById(id).get();
    }

    @DeleteMapping("/furniture/{id}")
    public void deleteById(@PathVariable String id){
        furnitureService.deleteById(id);
    }

    @PutMapping("/furniture")
    public void update(@RequestBody Furniture furniture){
        furnitureService.save(furniture);
    }
}
