package com.tfg_data_base.tfg.Marks;

import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;
import lombok.Data;

@Document(value = "Mark")
@Data
public class Mark {
    @Id
    private String id;
    private String name;
    private Double x;
    private Double y;

}
