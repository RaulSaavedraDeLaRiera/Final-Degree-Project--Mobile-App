package com.tfg_data_base.tfg.UserInfo;

import java.util.List;

import org.springframework.web.bind.annotation.*;
import lombok.RequiredArgsConstructor;

@RestController
@RequestMapping("/api/v1")
@RequiredArgsConstructor
public class UserInfoController {
    private final UserInfoService userInfoService;

    @GetMapping("/userinfo")
    public UserInfo getUserInfo() {
        return userInfoService.getUserInfo();
    }

    @PostMapping("/userinfo/users")
    public void addUser(@RequestParam String userId) {
        userInfoService.addUser(userId);
    }

    @DeleteMapping("/userinfo/users/{userId}")
    public void removeUser(@PathVariable String userId) {
        userInfoService.removeUser(userId);
    }

    @PostMapping("/userinfo/initialize")
    public String initializeUserInfo() {
        userInfoService.initializeEmptyUserInfo();
        return "UserInfo initialized with an empty user list.";
    }
}
