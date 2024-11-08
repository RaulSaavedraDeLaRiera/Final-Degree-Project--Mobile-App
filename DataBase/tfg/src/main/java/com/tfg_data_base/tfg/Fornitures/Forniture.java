package com.tfg_data_base.tfg.Fornitures;

import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;

import lombok.Data;

@Document(value = "Forniture")
@Data
public class Forniture {
@Id
    private String id;
    private String name;
    private String mesh;
    private Integer price;
    private Float percent;
    private Integer type;
    private Float speedDuration;
}
