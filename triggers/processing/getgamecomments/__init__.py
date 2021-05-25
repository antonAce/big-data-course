import json
import logging
import azure.functions as func

from ..db import *


def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('HTTP get comments of game requested.')
    game_id = req.params.get('game_id')

    if game_id is None:
        return func.HttpResponse(
            json.dumps({"message": "Game id is missing."}),
            status_code=400
        )

    filter_comments_query = SimpleStatement("""
        SELECT id, app_id, nickname, date_time, containment, reaction
        FROM gamestore.comments
        WHERE app_id = %s ALLOW FILTERING
    """, consistency_level=ConsistencyLevel.QUORUM)
    comments = session.execute(filter_comments_query, (int(game_id),)).all()

    return func.HttpResponse(
        json.dumps({
            "comments": [{
                "id": str(comment.id),
                "app_id": comment.app_id,
                "nickname": comment.nickname,
                "date_time": str(comment.date_time),
                "containment": comment.containment,
                "reaction": comment.reaction
            } for comment in comments]
        }),
        status_code=200
    )
