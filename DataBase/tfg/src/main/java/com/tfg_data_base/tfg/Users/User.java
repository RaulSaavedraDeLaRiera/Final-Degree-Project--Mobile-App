package com.tfg_data_base.tfg.Users;

import java.util.List;

import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;

import lombok.Data;

@Document(value = "User")
@Data
public class User {
    @Id
    private String id;
    private UserData userData;
    private List<SocialStat> socialStats;
    private PersonalStats personalStats;
    private List<Critteron> critterons;
    private List<FornitureOwned> fornitureOwned;

    @Data
    public static class UserData {
        private String name;
        private Integer level;
        private Integer experience;
        private Integer money;
        private String currentCritteron;
    }

    @Data
    public static class SocialStat {
        private String friendID;
    }

    @Data
    public static class PersonalStats {
        private Integer globalSteps;
        private Integer daysStreak;
        private Integer weekSteps;
        private Integer combatWins;
        private Integer critteronsOwned;
        private Integer percentHotel;
    }

    @Data
    public static class Critteron {
        private String critteronID;
        private Integer level;
        private Float currentLife;
        private StartInfo startInfo;

        @Data
        public static class StartInfo {
            private Integer stepAsPartner;
            private Integer usedAttacks;
            private Integer combatWins;
            private Integer timeAnaerobic;
            private Integer stepsDoingParade;
        }
    }

    @Data
    public static class FornitureOwned {
        private String fornitureID;
    }
}