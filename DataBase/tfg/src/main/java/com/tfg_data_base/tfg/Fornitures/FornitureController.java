package com.tfg_data_base.tfg.Fornitures;

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
public class FornitureController {

    private final FornitureService fornitureService;

    @PostMapping("/forniture")
    public void save(@RequestBody Forniture forniture){
        fornitureService.save(forniture);
    }

    @GetMapping("/forniture")
    public List<Forniture> findAll(){
        return fornitureService.findAll();
    }
    
    @GetMapping("/forniture/{id}")
    public Forniture findById(@PathVariable String id){
        return fornitureService.findById(id).get();
    }

    @DeleteMapping("/forniture/{id}")
    public void deleteById(@PathVariable String id){
        fornitureService.deleteById(id);
    }

    @PutMapping("/forniture")
    public void update(@RequestBody Forniture forniture){
        fornitureService.save(forniture);
    }
}
