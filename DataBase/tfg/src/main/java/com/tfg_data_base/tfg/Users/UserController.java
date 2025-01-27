package com.tfg_data_base.tfg.Users;

import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.tfg_data_base.tfg.Users.Bodys.UserFriend;
import com.tfg_data_base.tfg.Users.Bodys.UserUpdateRequest;

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

    @GetMapping("/login")
    public String login(@RequestBody User user) {
        return userService.login(user.getMail(), user.getPassword());
    }

    @GetMapping("/id")
    public String getId(@RequestBody User user) {
        return userService.getUserIdByCredentials(user.getMail(), user.getPassword());
    }

    @GetMapping("/user")
    public List<User> findAll() {
        return userService.findAll();
    }

    @GetMapping("/userTop")
    public List<String> findTop() {
        return userService.getTopThreeUsersByLevel();
    }

    @GetMapping("/userTopFriends/{id}")
    public List<String> getTopThreeFriendsByLevel(@PathVariable String id) {
        return userService.getTopThreeFriendsByLevel(id);
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

    @PatchMapping("/userPending/{id}")
    public void updateFriendPending(@PathVariable String id, @RequestBody UserUpdateRequest userUpdateRequest) {
        userService.updateUserPendingFriend(id, userUpdateRequest.getFieldName(), userUpdateRequest.getNewValue());
    }

    @DeleteMapping("/user/removeFriend/{id}")
    public void removeFriend(@PathVariable String id, @RequestBody UserFriend friendID) {
        userService.removeFriend(id, friendID.getFriendID());
    }

    @DeleteMapping("/user/removeFriendPending/{id}")
    public void removeFriendPending(@PathVariable String id, @RequestBody UserFriend friendID) {
        userService.removeFriendPending(id, friendID.getFriendID());
    }

    @PatchMapping("/userSent/{id}")
    public void updateFriendSent(@PathVariable String id, @RequestBody UserUpdateRequest userUpdateRequest) {
        userService.updateUserSentFriend(id, userUpdateRequest.getFieldName(), userUpdateRequest.getNewValue());
    }

    @DeleteMapping("/user/removeFriendSent/{id}")
    public void removeFriendSent(@PathVariable String id, @RequestBody UserFriend friendID) {
        userService.removeFriendSent(id, friendID.getFriendID());
    }
}
