package com.tfg_data_base.tfg.UserInfo;

import java.util.ArrayList;
import java.util.List;
import org.springframework.data.mongodb.core.mapping.Document;

import lombok.Data;

@Document(value = "UserInfo")
@Data
public class UserInfo {

    private String id = "USER_INFO_ID";
    private List<User> users = new ArrayList<>();

    @Data
    public static class User {
        private String userID;
    }
}

