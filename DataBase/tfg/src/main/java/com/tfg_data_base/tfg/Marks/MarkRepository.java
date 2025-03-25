package com.tfg_data_base.tfg.Marks;

import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface MarkRepository extends MongoRepository<Mark, String> {

}
