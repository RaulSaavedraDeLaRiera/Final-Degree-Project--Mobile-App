package com.tfg_data_base.tfg.GameInfo;

import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface GameInfoRepository extends MongoRepository<GameInfo, String> {
}
