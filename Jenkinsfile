node {
    stage('import') {
        try {
            git 'https://github.com/1804-Apr-USFdotnet/Project2-BrewTodo.git'
        } catch(exc) {
            slackError('import')
            throw exc
        }
    }
    stage('build') {
        try {
            dir('BrewTodoServer') {
                bat 'nuget restore'
                bat 'msbuild /p:MvCBuildViews=true'
            }
        } catch(exc) {
            slackError('build')
            throw exc
        }
    }
    stage('test') {
        try {
            dir('BrewTodoServer') {
                //TODO add unit test project
                //bat "VSTest.Console.exe BrewTodoServer.Tests\\bin\\Debug\\BrewTodoServer.Tests.dll"
            }
        } catch(exc) {
            slackError('test')
            throw exc
        } 
    }
    stage('analyze') {
        try {
            dir('BrewTodoServer') {
                bat 'SonarQube.Scanner.MSBuild.exe begin /k:\"brewtodo-server-key\" /d:sonar.organization=\"brewtodo-group\" /d:sonar.host.url=\"https://sonarcloud.io\"'
                bat 'msbuild /t:rebuild'
                bat 'SonarQube.Scanner.MSBuild.exe end'
            }
        } catch(exc) {
            slackError('analyze')
            throw exc
        }
    }
    stage('package') {
        try {
            dir('BrewTodoServer') {
                bat 'msbuild BrewTodoServer /t:package'
            }
        } catch(exc) {
            slackError('package')
            throw exc
        }
    }
    stage('deploy to build server') {
        try {
            dir('BrewTodoServer\\BrewTodoServer\\obj\\Debug\\Package') {
                bat "\"C:\\Program Files\\IIS\\Microsoft Web Deploy V3\\msdeploy.exe\" -source:package='C:\\Program Files (x86)\\Jenkins\\workspace\\BrewTodo\\BrewTodoServer\\BrewTodoServer\\obj\\Debug\\Package\\BrewTodoServer.zip' -dest:auto,computerName=\"https://ec2-18-222-53-65.us-east-2.compute.amazonaws.com:8172/msdeploy.axd\",userName=\"Administrator\",password=\"${env.server_deploy_password}\",authtype=\"basic\",includeAcls=\"False\" -verb:sync -disableLink:AppPoolExtension -disableLink:ContentExtension -disableLink:CertificateExtension -setParamFile:\"C:\\Program Files (x86)\\Jenkins\\workspace\\BrewTodo\\BrewTodoServer\\BrewTodoServer\\obj\\Debug\\Package\\BrewTodoServer.SetParameters.xml\" -AllowUntrusted=True"
            }
        } catch(exc) {
            slackError('deploy')
            throw exc
        }
    }
    stage('deploy to dev server') {
        try {
            dir('BrewTodoServer\\BrewTodoServer\\obj\\Debug\\Package') {
                bat "\"C:\\Program Files\\IIS\\Microsoft Web Deploy V3\\msdeploy.exe\" -source:package='C:\\Program Files (x86)\\Jenkins\\workspace\\BrewTodo\\BrewTodoServer\\BrewTodoServer\\obj\\Debug\\Package\\BrewTodoServer.zip' -dest:auto,computerName=\"https://ec2-18-222-156-248.us-east-2.compute.amazonaws.com:8172/msdeploy.axd\",userName=\"Administrator\",password=\"${env.dev_deploy_password}\",authtype=\"basic\",includeAcls=\"False\" -verb:sync -disableLink:AppPoolExtension -disableLink:ContentExtension -disableLink:CertificateExtension -setParamFile:\"C:\\Program Files (x86)\\Jenkins\\workspace\\BrewTodo\\BrewTodoServer\\BrewTodoServer\\obj\\Debug\\Package\\BrewTodoServer.SetParameters.xml\" -AllowUntrusted=True"
            }
        } catch(exc) {
            slackError('deploy')
            throw exc
        }
    }
    stage('success') {
        slackSend color: 'good', message: "Pipeline end reached. [<${JOB_URL}|${env.JOB_NAME}> <${env.BUILD_URL}console|${env.BUILD_DISPLAY_NAME}>] [${currentBuild.durationString[0..-14]}]"
    }
}

def slackError(stageName) {
    slackSend color: 'danger', message: "${stageName} stage failed. [<${JOB_URL}|${env.JOB_NAME}> <${env.BUILD_URL}console|${env.BUILD_DISPLAY_NAME}>] [${currentBuild.durationString[0..-14]}]"
}