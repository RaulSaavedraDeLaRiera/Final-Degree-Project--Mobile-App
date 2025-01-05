package com.tfg_data_base.tfg.Critterons;

import java.util.List;
import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;
import lombok.Data;

@Document(value = "Critteron")
@Data
public class Critteron {
    @Id
    private String id;
    private String name;
    private String mesh;
    private Integer levelUnlock;
    private Integer life;
    private Integer basicDamage;
    private Integer defense;
    private Integer idZone;
    private List<Attack> attacks;

    @Data
    public static class Attack {
        private String name;
        private Integer damage;
        private Float percent;
        private Integer type;

        public Attack(String name, Integer damage, Float percent, Integer type) {
            this.name = name;
            this.damage = damage;
            this.percent = percent;
            this.type = type;
        }
    }
}
