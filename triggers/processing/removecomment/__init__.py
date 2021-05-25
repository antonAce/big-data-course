import json
import logging
import azure.functions as func
from ..db import *


def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('HTTP comment removing requested.')

    comment_id = req.params.get('id')

    if comment_id is None:
        return func.HttpResponse(
            json.dumps({"message": "Game id is missing."}),
            status_code=400
        )

    delete_comment_by_query = SimpleStatement("""
        DELETE FROM gamestore.comments WHERE id = %s
    """, consistency_level=ConsistencyLevel.QUORUM)

    session.execute(delete_comment_by_query, (comment_id,))

    return func.HttpResponse(
        json.dumps({
            "id": comment_id
        })
    )
