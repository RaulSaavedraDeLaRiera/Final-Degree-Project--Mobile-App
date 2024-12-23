package com.tfg_data_base.tfg.Critterons;

import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface CritteronRepository extends MongoRepository<Critteron, String> {

}
