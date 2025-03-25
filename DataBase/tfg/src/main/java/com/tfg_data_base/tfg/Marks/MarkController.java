package com.tfg_data_base.tfg.Marks;

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
public class MarkController {

    private final MarkService markService;

    @PostMapping("/mark")
    public void save(@RequestBody Mark mark) {
        markService.save(mark);
    }

    @GetMapping("/mark")
    public List<Mark> findAll() {
        return markService.findAll();
    }

    @GetMapping("/mark/{id}")
    public Mark findById(@PathVariable String id) {
        return markService.findById(id).get();
    }

    @DeleteMapping("/mark/{id}")
    public void deleteById(@PathVariable String id) {
        markService.deleteById(id);
    }

    @PutMapping("/mark")
    public void update(@RequestBody Mark mark) {
        markService.save(mark);
    }
}
