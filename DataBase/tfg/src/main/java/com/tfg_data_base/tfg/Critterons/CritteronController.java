package com.tfg_data_base.tfg.Critterons;

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
public class CritteronController {

    private final CritteronService critteronService;

    @PostMapping("/critteron")
    public void save(@RequestBody Critteron critteron){
        critteronService.save(critteron);
    }

    @GetMapping("/critteron")
    public List<Critteron> findAll(){
        return critteronService.findAll();
    }
    
    @GetMapping("/critteron/{id}")
    public Critteron findById(@PathVariable String id){
        return critteronService.findById(id).get();
    }

    @DeleteMapping("/critteron/{id}")
    public void deleteById(@PathVariable String id){
        critteronService.deleteById(id);
    }

    @PutMapping("/critteron")
    public void update(@RequestBody Critteron critteron){
        critteronService.save(critteron);
    }
}
