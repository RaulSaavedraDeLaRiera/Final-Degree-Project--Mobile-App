package com.tfg_data_base.tfg.Fornitures;

import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface FornitureRepository extends MongoRepository<Forniture, String>{

}
