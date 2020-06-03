import json
import requests
import argparse

def get_parser():

    """

    Generate a parameters parser.

    """

    # parse parameters
    parser = argparse.ArgumentParser(description="Update job priority V2")

    # job cluster and authorization
    parser.add_argument("--cluster", type=str, default="", help="DLTS cluster")
    parser.add_argument("--team", type=str, default="", help="DLTS team")
    parser.add_argument("--userName", type=str, default="", help="Username in DLTS personal page. Your own alias")

    # job info
    parser.add_argument("--jobId", type=str, default="", help="Id of the job")
    parser.add_argument("--priority", type=int, default="", help="Job priority value")

    return parser

def parametersValidation(params):

    # please add more if like
    params_not_None = [params.cluster, params.team, params.userName, params.jobId]
    for param in params_not_None:
        assert param is not None and len(str(param)) != 0


def query_job(params):
    put_url = 'http://itpaether.azurewebsites.net/{0}/jobs/priorities?userName={1}@microsoft.com'.format(params.cluster, params.userName)

    jobparams = {
        params.jobId: params.priority
    }
    
    print('url = {}'.format(put_url))
    print('jobparams: type={} value = {}'.format(type(jobparams), jobparams))
    
    r = requests.post(
        url=put_url,
        json=jobparams)

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