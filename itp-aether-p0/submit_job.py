import json
import requests
import argparse


def str2bool(v):
    if v.lower() in ('yes', 'true', 't', 'y', '1'):
        return True
    elif v.lower() in ('no', 'false', 'f', 'n', '0'):
        return False
    else:
        raise argparse.ArgumentTypeError('Boolean value expected.')


def get_parser():

    """

    Generate a parameters parser.

    """

    # parse parameters
    parser = argparse.ArgumentParser(description="Submit jobs to DLTS V2")

    # job cluster and authorization
    parser.add_argument("--cluster", type=str, default="", help="DLTS cluster")
    parser.add_argument("--team", type=str, default="", help="DLTS VC/team")
    parser.add_argument("--userName", type=str, default="", help="Username in DLTS personal page. Your own alias")

    # job info
    parser.add_argument("--jobName", type=str, default="Aether submitted job", help="Name of job")
    parser.add_argument("--jobTrainingType", type=str, default="RegularJob", help="RegularJob or PSDistJob")
    parser.add_argument("--resourceGpu", type=int, default=0, help="GPUs for RegularJob, or GPUs per node for PSDistJob")
    parser.add_argument("--nodes", type=int, default=1, help="Number of nodes for PSDistJob")
    parser.add_argument("--gpuType", type=str, default="", help="Gpu type")
    parser.add_argument("--preemptionAllowed", type=str2bool, default=False, help="Whether to allow preemption on this job")
    parser.add_argument("--workPath", type=str, default="./", help="Relative work path based on //$cluster-nfs/")
    parser.add_argument("--dataPath", type=str, default="./", help="Relative data path based on //$cluster-nfs/")
    parser.add_argument("--jobPath", type=str, default="", help="Relative job path based on //$cluster-nfs/")
    parser.add_argument("--image", type=str, default="indexserveregistry.azurecr.io/deepscale:1.0.post0", help="Docker image to run the job")
    parser.add_argument("--command", type=str, default="", help="Command to run")
    parser.add_argument("--priority", type=int, default=100, help="Job priority")
    
    return parser


def parametersValidation(params):
    # team and userName must be provided
    params_not_none = [params.team, params.userName]
    for param in params_not_none:
        assert param is not None and len(str(param)) != 0


def get_regular_job_params(params):
    job_params = {
        "userName": params.userName + "@microsoft.com",
        "jobName":  params.jobName + " from Aether module (regular job)",
        "preemptionAllowed": params.preemptionAllowed,
        "vcName": params.team,
        "resourcegpu": params.resourceGpu,
        "gpuType": params.gpuType,
        "workPath": params.workPath,
        "enableworkpath": True,
        "dataPath": params.dataPath,
        "enabledatapath": True,
        "jobPath": params.jobPath,
        "enablejobpath": True,
        "image": params.image,
        "cmd": params.command,
        "jobType": "training",
        "jobtrainingtype": params.jobTrainingType
    }
    return job_params


def get_dist_job_params(params):
    # get GPU number from vc info
    get_url = 'http://itpaether.azurewebsites.net/{0}/ListVCs?userName={1}@microsoft.com'.format(params.cluster, params.userName)
    r = requests.get(url=get_url)
    rb = json.loads(r.text)
    print('vc info:{}'.format(r.text))
    quota = json.loads(rb['result'][0]['quota'])
    for k, v in quota.items():
        if k == params.gpuType:
            params.resourceGpu = v

    job_params = {
        "userName": params.userName + "@microsoft.com",
        "jobName":  params.jobName + " from Aether module (distributed job)",
        "preemptionAllowed": params.preemptionAllowed,
        "vcName": params.team,
        "resourcegpu": params.resourceGpu, # Make sure to use the entire node. Unpredictable behavior will occur if not.
        "numps": 1, # Keep 1.
        "numpsworker": params.nodes,
        "hostNetwork": True, # Use hostnetwork for distributed jobs. Will be deprecated soon and handled in DLTS scheduler.
        "isPrivileged": True, # Use privileged container for distributed jobs. Will be deprecated soon and handled in DLTS scheduler.
        "gpuType": params.gpuType,
        "workPath": params.workPath,
        "enableworkpath": True,
        "dataPath": params.dataPath,
        "enabledatapath": True,
        "jobPath": params.jobPath,
        "enablejobpath": True,
        "image": params.image,
        "cmd": params.command,
        "jobType": "training",
        "jobtrainingtype": params.jobTrainingType
    }
    return job_params


def submit_job(params):
    submit_url = 'http://itpaether.azurewebsites.net/{0}/PostJob'.format(params.cluster)

    if params.jobTrainingType == "RegularJob":
        jobparams = get_regular_job_params(params)
    elif params.jobTrainingType == "PSDistJob":
        jobparams = get_dist_job_params(params)
    else:
        raise ValueError("jobTrainingType is not recognized")

    if params.priority != 100:
        jobparams["jobPriority"] = params.priority

    print('url = {}'.format(submit_url))

    print('jobparams: type={} value = {}'.format(type(jobparams), jobparams))
    print('payload = {}'.format(jobparams))

    r = requests.post(
        url=submit_url,
        json=jobparams)

    print('response={}'.format(r))
    print('response_text={}'.format(r.text))


if __name__ == '__main__':
    # generate parser / parse parameters
    parser = get_parser()
    params = parser.parse_args()

    # validate params
    parametersValidation(params)

    # submit job
    submit_job(params)