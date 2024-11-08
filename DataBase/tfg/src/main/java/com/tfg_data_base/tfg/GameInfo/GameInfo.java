package com.tfg_data_base.tfg.GameInfo;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;

import lombok.Data;

@Document(value = "GameInfo")
@Data
public class GameInfo {
    @Id
    private String id; 
    private List<Critteron> critterons = new ArrayList<>();
    private List<Forniture> forniture = new ArrayList<>();
    private List<User> users = new ArrayList<>();
    private WeekRewards weekRewards;

    @Data
    public static class Critteron {
        private String critteronID;
    }

    @Data
    public static class Forniture {
        private String fornitureID;
    }

    @Data
    public static class User {
        private String userID;
    }

    @Data
    public static class WeekRewards {
        private Map<String, DayReward> days;

        @Data
        public static class DayReward {
            private Integer reward1;
            private Integer reward2;

            public DayReward(Integer reward1, Integer reward2) {
                this.reward1 = reward1;
                this.reward2 = reward2;
            }
        }
    }
}
