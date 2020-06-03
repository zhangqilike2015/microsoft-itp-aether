import json
import requests
import argparse


def get_parser():

    """

    Generate a parameters parser.

    """

    # parse parameters
    parser = argparse.ArgumentParser(description="Query jobs detail V2")

    # job cluster and authorization
    parser.add_argument("--cluster", type=str, default="", help="DLTS cluster")
    parser.add_argument("--team", type=str, default="", help="DLTS eam")
    parser.add_argument("--userName", type=str, default="", help="Username in DLTS personal page. Your own alias")

    # job info
    parser.add_argument("--jobId", type=str, default="", help="Id of the job")
    parser.add_argument("--status", type=str, default="", help="To approve an unapproved job: \"ApproveJob\". "
                                                               "To kill a job: \"KillJob\". "
                                                               "To pause a running job: \"PauseJob\". "
                                                               "To resume a paused job: \"ResumeJob\".")
    return parser

def parametersValidation(params):

    # please add more if like
    params_not_None = [params.cluster, params.team, params.userName, params.jobId]
    for param in params_not_None:
        assert param is not None and len(str(param)) != 0


def query_job(params):
    get_url = 'http://itpaether.azurewebsites.net/{0}/{1}?userName={2}@microsoft.com&jobId={3}'.format(params.cluster, params.status, params.userName, params.jobId)

    print('url = {}'.format(get_url))

    r = requests.get(url=get_url)

    print('response={}'.format(r))
    print('response_text={}'.format(r.text))


if __name__ == '__main__':
    # generate parser / parse parameters
    parser = get_parser()
    params = parser.parse_args()

    # validate params
    parametersValidation(params)

    # query job
    query_job(params)