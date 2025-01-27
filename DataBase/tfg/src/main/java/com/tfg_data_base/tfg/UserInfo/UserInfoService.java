package com.tfg_data_base.tfg.UserInfo;

import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

import org.springframework.data.mongodb.core.query.Query;
import org.springframework.stereotype.Service;

import lombok.RequiredArgsConstructor;

@Service
@RequiredArgsConstructor
public class UserInfoService {
    private final UserInfoRepository userInfoRepository;

    public UserInfo getUserInfo() {
        return userInfoRepository.findById("USER_INFO_ID")
                .orElseGet(() -> {
                    UserInfo newUserInfo = new UserInfo();
                    newUserInfo.setId("USER_INFO_ID");
                    newUserInfo.setUsers(new ArrayList<>());
                    userInfoRepository.save(newUserInfo);
                    return newUserInfo;
                });
    }

    public void addUser(String userID) {
        UserInfo userInfo = getUserInfo();

        boolean exists = userInfo.getUsers().stream()
                .anyMatch(user -> user.getUserID().equals(userID));

        if (!exists) {
            UserInfo.User newUser = new UserInfo.User();
            newUser.setUserID(userID);
            userInfo.getUsers().add(newUser);
            userInfoRepository.save(userInfo);
        } else {
            System.out.println("User with ID " + userID + " already exists in the data base");
        }
    }

    public void removeUser(String userID) {
        UserInfo userInfo = getUserInfo();
        userInfo.getUsers().removeIf(user -> user.getUserID().equals(userID));
        userInfoRepository.save(userInfo);
    }

    public void initializeEmptyUserInfo() {
        UserInfo userInfo = userInfoRepository.findById("USER_INFO_ID")
                .orElse(null);

        if (userInfo == null) {
            UserInfo emptyUserInfo = new UserInfo();
            emptyUserInfo.setId("USER_INFO_ID");
            emptyUserInfo.setUsers(new ArrayList<>());
            userInfoRepository.save(emptyUserInfo);
        }
    }

}
