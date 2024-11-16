package com.tfg_data_base.tfg.Users;

import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import lombok.RequiredArgsConstructor;

import java.util.List;

import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PatchMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;

@RestController
@RequestMapping("/api/v1")
@RequiredArgsConstructor
public class UserController {

    private final UserService userService;

    @PostMapping("/newUser")
    public void save(@RequestBody User user) {
        userService.save(user); 
    }

    @GetMapping("/user")
    public List<User> findAll() {
        return userService.findAll();
    }
    
    @GetMapping("/user/{id}")
    public User findById(@PathVariable String id) {
        return userService.findById(id).orElse(null);
    }

    @DeleteMapping("/user/{id}")
    public void deleteById(@PathVariable String id) {
        userService.deleteById(id);
    }

    @PutMapping("/user")
    public void update(@RequestBody User user) {
        userService.save(user);
    }

    @PatchMapping("/user/{id}")
    public void updateUserField(@PathVariable String id, @RequestBody UserUpdateRequest userUpdateRequest) {
        userService.updateUserField(id, userUpdateRequest.getFieldName(), userUpdateRequest.getNewValue());
    }
}
